// 6. return when done in Function 

basePtr* getObjPtr (int id) {
    basePtr* local = 0;
    
    if (id = = 1) local = new Child;
    else if (id = = 2) local = new Child2;
    else if (id = = 3) local = new Child3;
    else if (id = = 4) local = new GChild2a;
    else if (id = = 5) local = new GChild2b;
    else if (id = = 6) local = new GChild3a;
    
    return local;
}