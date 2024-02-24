#define cde_c

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include "cde.h"
#include "cde_lexer.h"

int run_source_file(char *cde_source) {
    char current_directory[256];
    getcwd(current_directory, sizeof(current_directory));

    FILE *p_cde_source = fopen(cde_source, "r");
    if (p_cde_source == NULL) {
        // printf("\033[0;31m");
        printf("Can't open file: %s\\%s\n", current_directory, cde_source);
        printf("Make sure such file or directory exists\n");
        // printf("\033[0m");
        fclose(p_cde_source);
    } else {
        printf("Passed");
        fclose(p_cde_source);
    }
    return CDE_OK;
}

int run_interactive_mode() {
    printf("CODE 0.1.0 by Hitsuji Labs - https://github.com/ShySpaceSheep/CODE-interpreter\nType \"help\" or \"credits\" for more information.\n");
    char line[MAX_BUFFER_SIZE];

    for (;;) {
        printf(">>> ");

        // Use fgets to read a line
        if (fgets(line, sizeof(line), stdin) == NULL) {
            break; // Break on end of input or error
        }

        // Remove newline character at the end
        size_t len = strlen(line);
        if (len > 0 && line[len - 1] == '\n') {
            line[len - 1] = '\0';
        }
    }
    return CDE_OK;
}

int main(int argc, char **argvc) {
    int status;

    if (argc > 2) {
        // printf("\033[0;31m");
        printf("Invalid arguments, refer below to get started\nUsage: cde [CODE source file] or cde.exe [CODE source file]\n");
        // printf("\033[0m");
        return EXIT_FAILURE;
    } else if (argc == 2) {
        status = run_source_file(argvc[1]);
    } else {
        status = run_interactive_mode();
    }
    return (status == CDE_OK) ? EXIT_SUCCESS : EXIT_FAILURE;
}