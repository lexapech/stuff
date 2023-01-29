#pragma once
#include <iostream>
#include "Vector.h"
#define _USE_MATH_DEFINES
#include "math.h"

class Bike;
class Wheel;
struct spring
{
	double k;
	double d;
	double l;
	Bike* bike;
	Vector2 p1;
	Wheel* p2;
	Vector2 dir2;
	Vector2 force1() const;
	Vector2 force2() const;
	Vector2 dir() const;

	Vector2 mid() const;

	double len() const;
	double angle() const;
};



struct Body
{
	Vector2 pos;
	Vector2 momentum;
	double angle;
	double L;
};
struct Wheel
{
	spring s;
	Body b;
	double mass;
	double radius;
};


class Bike
{
public:
	Body b;
	Wheel rear;
	Wheel front;
	double mass;
	Bike operator+(Bike r);
	Bike operator*(double k);
	Bike();
	void update(int controls);

	Vector2 toGlobal(Vector2 v) const;
	Vector2 rotate(Vector2 v, double angle) const;
	Vector2 toLocal(Vector2 v) const;
	static void func(Bike* out, const Bike& in);

	static Bike rk(Bike* b);
	bool right;
	bool left;
	bool brake;
};


