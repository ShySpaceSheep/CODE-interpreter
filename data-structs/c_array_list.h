#ifndef C_ARRLIST_H
#define C_ARRLIST_H

#include <stdlib.h>

typedef struct ArrayList {
    void **list;
    size_t size;
    size_t capacity;
} ArrayList;

struct ArrayList *createArrList();
void destroyArrList(ArrayList *list);
void arrListAdd(ArrayList *list, void *data);
void arrListInsert(ArrayList *list, int index, void *data);
void arrListRemove(ArrayList *list, int index);
void *arrListGet(ArrayList *list, int index);
size_t arrListSize(ArrayList *list);

#endif