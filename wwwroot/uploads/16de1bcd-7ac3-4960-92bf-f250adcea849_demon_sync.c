#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/mman.h>
#include <fcntl.h>
#include <dirent.h>
#include <string.h>
#include <errno.h>
#include <time.h>
#include <signal.h>
#include <syslog.h>
#include <limits.h>
#include <stdarg.h>

#define DOMYSLNY_CZAS_SNU 300
#define DOMYSLNY_PROG_ROZMIARU 1024 * 1024 // 1MB

int rekurencyjnie = 0;
int czas_snu = DOMYSLNY_CZAS_SNU;
off_t prog_rozmiaru = DOMYSLNY_PROG_ROZMIARU;
volatile sig_atomic_t obudz_sie = 0;

void obsluga_sigusr1(int znak) {
    obudz_sie = 1;
}

void loguj_wiadomosc(const char* format, ...) {
    va_list args;
    va_start(args, format);
    vsyslog(LOG_INFO, format, args);
    va_end(args);
}

time_t pobierz_czas_modyfikacji(const char* sciezka) {
    struct stat st;
    if (stat(sciezka, &st) == 0)
        return st.st_mtime;
    return 0;
}

int czy_plik_zwykly(const char* sciezka) {
    struct stat st;
    return stat(sciezka, &st) == 0 && S_ISREG(st.st_mode);
}

int czy_katalog(const char* sciezka) {
    struct stat st;
    return stat(sciezka, &st) == 0 && S_ISDIR(st.st_mode);
}

int kopiuj_plik(const char* zrodlowy, const char* docelowy, off_t prog) {
    int fd_zrodlowy = open(zrodlowy, O_RDONLY);
    if (fd_zrodlowy < 0) return -1;

    struct stat st;
    fstat(fd_zrodlowy, &st);
    off_t rozmiar = st.st_size;

    int fd_docelowy = open(docelowy, O_WRONLY | O_CREAT | O_TRUNC, 0644);
    if (fd_docelowy < 0) {
        close(fd_zrodlowy);
        return -1;
    }

    if (rozmiar > prog) {
        void* mapa = mmap(NULL, rozmiar, PROT_READ, MAP_PRIVATE, fd_zrodlowy, 0);
        if (mapa == MAP_FAILED) {
            close(fd_zrodlowy);
            close(fd_docelowy);
            return -1;
        }
        write(fd_docelowy, mapa, rozmiar);
        munmap(mapa, rozmiar);
    }
    else {
        char bufor[8192];
        ssize_t bajty;
        while ((bajty = read(fd_zrodlowy, bufor, sizeof(bufor))) > 0)
            write(fd_docelowy, bufor, bajty);
    }

    struct timespec czasy[2] = { {0, 0}, {st.st_mtime, 0} };
    futimens(fd_docelowy, czasy);

    close(fd_zrodlowy);
    close(fd_docelowy);
    return 0;
}

void usun_rekurencyjnie(const char* sciezka) {
    DIR* katalog = opendir(sciezka);
    if (!katalog) return;
    struct dirent* wpis;
    char pelna_sciezka[PATH_MAX];
    while ((wpis = readdir(katalog)) != NULL) {
        if (!strcmp(wpis->d_name, ".") || !strcmp(wpis->d_name, "..")) continue;
        snprintf(pelna_sciezka, PATH_MAX, "%s/%s", sciezka, wpis->d_name);
        if (wpis->d_type == DT_DIR) {
            usun_rekurencyjnie(pelna_sciezka);
        }
        else {
            unlink(pelna_sciezka);
        }
    }
    closedir(katalog);
    rmdir(sciezka);
}

void synchronizuj_katalogi(const char* src, const char* dst) {
    DIR* katalog_src = opendir(src);
    if (!katalog_src) return;

    if (!opendir(dst)) {
        mkdir(dst, 0755);
    }

    struct dirent* wpis;
    char sciezka_src[PATH_MAX], sciezka_dst[PATH_MAX];
    struct stat st;

    while ((wpis = readdir(katalog_src)) != NULL) {
        if (!strcmp(wpis->d_name, ".") || !strcmp(wpis->d_name, "..")) continue;
        snprintf(sciezka_src, PATH_MAX, "%s/%s", src, wpis->d_name);
        snprintf(sciezka_dst, PATH_MAX, "%s/%s", dst, wpis->d_name);

        if (lstat(sciezka_src, &st) < 0) continue;

        if (S_ISREG(st.st_mode)) {
            if (!czy_plik_zwykly(sciezka_dst) || pobierz_czas_modyfikacji(sciezka_src) > pobierz_czas_modyfikacji(sciezka_dst)) {
                if (kopiuj_plik(sciezka_src, sciezka_dst, prog_rozmiaru) == 0) {
                    loguj_wiadomosc("Skopiowano plik: %s -> %s", sciezka_src, sciezka_dst);
                }
            }
        }
        else if (S_ISDIR(st.st_mode) && rekurencyjnie) {
            synchronizuj_katalogi(sciezka_src, sciezka_dst);
        }
    }
    closedir(katalog_src);

    // Usuwanie nieistniej¹cych plików z katalogu docelowego
    DIR* katalog_dst = opendir(dst);
    if (!katalog_dst) return;

    while ((wpis = readdir(katalog_dst)) != NULL) {
        if (!strcmp(wpis->d_name, ".") || !strcmp(wpis->d_name, "..")) continue;
        snprintf(sciezka_dst, PATH_MAX, "%s/%s", dst, wpis->d_name);
        snprintf(sciezka_src, PATH_MAX, "%s/%s", src, wpis->d_name);

        if (lstat(sciezka_dst, &st) < 0) continue;

        if (access(sciezka_src, F_OK) != 0) {
            if (S_ISREG(st.st_mode)) {
                unlink(sciezka_dst);
                loguj_wiadomosc("Usuniêto plik: %s", sciezka_dst);
            }
            else if (S_ISDIR(st.st_mode) && rekurencyjnie) {
                usun_rekurencyjnie(sciezka_dst);
                loguj_wiadomosc("Usuniêto katalog: %s", sciezka_dst);
            }
        }
    }
    closedir(katalog_dst);
}

void demonizuj() {
    pid_t pid = fork();
    if (pid < 0) exit(1);
    if (pid > 0) exit(0);

    setsid();

    pid = fork();
    if (pid < 0) exit(1);
    if (pid > 0) exit(0);

    umask(0);
    chdir("/");

    close(STDIN_FILENO);
    close(STDOUT_FILENO);
    close(STDERR_FILENO);
}

int main(int argc, char* argv[]) {
    if (argc < 3) {
        fprintf(stderr, "U¿ycie: %s <katalog_Ÿród³owy> <katalog_docelowy> [-R] [czas_snu] [próg_rozmiaru]\n", argv[0]);
        return 1;
    }

    const char* src = argv[1];
    const char* dst = argv[2];

    for (int i = 3; i < argc; ++i) {
        if (strcmp(argv[i], "-R") == 0) rekurencyjnie = 1;
        else if (czas_snu == DOMYSLNY_CZAS_SNU) czas_snu = atoi(argv[i]);
        else prog_rozmiaru = atol(argv[i]);
    }

    if (!czy_katalog(src) || !czy_katalog(dst)) {
        fprintf(stderr, "Obie œcie¿ki musz¹ byæ katalogami.\n");
        return 1;
    }

    signal(SIGUSR1, obsluga_sigusr1);
    openlog("demon_sync", LOG_PID | LOG_CONS, LOG_DAEMON);
    loguj_wiadomosc("Demon uruchomiony. ród³o: %s, Cel: %s", src, dst);

    demonizuj();

    while (1) {
        loguj_wiadomosc("Demon œpi przez %d sekund", czas_snu);
        for (int i = 0; i < czas_snu && !obudz_sie; ++i)
            sleep(1);
        obudz_sie = 0;
        loguj_wiadomosc("Demon budzi siê, aby zsynchronizowaæ katalogi");
        synchronizuj_katalogi(src, dst);
    }

    closelog();
    return 0;
}
