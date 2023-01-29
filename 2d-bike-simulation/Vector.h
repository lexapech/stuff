#pragma once
#include <cmath>
struct Vector2
{
	double x, y;
	Vector2(double x = 0, double y = 0);
	Vector2 operator/(const double r) const;
	Vector2 operator*(const double r) const;
	Vector2 operator+(const Vector2 r) const;
	Vector2 operator-(const Vector2 r) const;
	double len() const;
	static double dot(const Vector2 l, const Vector2 r);
};