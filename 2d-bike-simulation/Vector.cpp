#include "Vector.h"
Vector2::Vector2(double x, double y)
{
	this->x = x;
	this->y = y;
}
Vector2 Vector2::operator/(const double r) const
{
	return Vector2(x / r, y / r);
}
Vector2 Vector2::operator*(const double r) const
{
	return Vector2(x * r, y * r);
}
Vector2 Vector2::operator+(const Vector2 r) const
{
	return Vector2(x + r.x, y + r.y);
}
Vector2 Vector2::operator-(const Vector2 r) const
{
	return Vector2(x - r.x, y - r.y);
}
double Vector2::len() const
{
	return sqrt(x * x + y * y);
}
double Vector2::dot(const Vector2 l, const Vector2 r)
{
	return l.x * r.x + l.y * r.y;
}