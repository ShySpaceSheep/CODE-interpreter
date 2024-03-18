#ifndef C_ARRLIST
#define C_ARRLIST

#include "c_array_list.h"
#include <stdlib.h>

#define INITIAL_CAPACITY 10

struct ArrayList *createArrList() {
    ArrayList *arrList = (ArrayList *)malloc(sizeof(ArrayList));

    if (arrList == NULL) {
        perror("Failed to allocate memory to the list, terminating...");
        exit(EXIT_FAILURE);
    }

    arrList->list = (void *)malloc(INITIAL_CAPACITY * sizeof(void *));
    arrList->size = 0;
    arrList->capacity = INITIAL_CAPACITY;

    return arrList;
}

void destroyArrList(ArrayList *list) {
    free(list->list);
    free(list);
}

void arrListAdd(ArrayList *list, void *data) {
    if (list->size >= list->capacity) {
        list->capacity = (int)ceil(1.5 * list->capacity);
        list->list = (void **)realloc(list->list, list->capacity * sizeof(void *));
        if (list->list == NULL) {
            perror("Failed to re-allocate memory to the list, terminating...");
            exit(EXIT_FAILURE);
        }
    }
    list->list[list->size++] = data;
}

void arrListInsert(ArrayList *list, int index, void *data) {
}

void arrListRemove(ArrayList *list, int index) {
    if (index > list->size) {
        perror("Index out of bounds, terminating...");
        exit(EXIT_FAILURE);
    }

    for (size_t i = index; i < list->size - 1; i++) {
        list->list[i] = list->list[i + 1];
    }
    list->size--;
}

void *arrListGet(ArrayList *list, int index) {
    if (index > list->size) {
        perror("Index out of bounds, terminating...");
        exit(EXIT_FAILURE);
    }
    return list->list[index];
}

size_t arrListSize(ArrayList *list) {
    return list->size;
}

#endif