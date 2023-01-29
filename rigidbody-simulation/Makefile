compiler = g++ 
arg = -std=c++14 -IGL -IGLU 
LIBS=-lsfml-graphics -lsfml-window  -lsfml-system -lGL -lGLU
all: main.o func.o
	$(compiler) main.o func.o  $(arg) $(LIBS)
main.o: func.h main.cpp
	$(compiler) $(arg)  -c main.cpp
func.o: func.h func.cpp
	$(compiler) $(arg) -c func.cpp 	
	
