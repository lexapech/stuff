#pragma once
#include <vector>
#include <iostream>
#include <cstring>
#include <cstdlib>
#include <cmath>
#include <SFML/Window.hpp>
#include <SFML/OpenGL.hpp>
#include <GL/gl.h>
#include <GL/glu.h>




class Vector3d
{
public:
	double x, y, z;
	Vector3d() : x(0), y(0), z(0) {}
	Vector3d(double x, double y, double z) : x(x), y(y), z(z) {}
	Vector3d operator+(const Vector3d rv) const;
	Vector3d operator-(const Vector3d rv) const;
	Vector3d operator/(const double rv) const;
	Vector3d operator*(const double rv) const;
	static Vector3d cross(const Vector3d l, const Vector3d r);
	static double dot(const Vector3d l,const Vector3d r);
	double& operator[](int index);
	Vector3d normalize();
	Vector3d abs();
	double magnitude();
};

struct Quaternion
{
	double re;
	Vector3d im;
	Quaternion() :re(0), im(Vector3d()) {};
	Quaternion(double real, Vector3d img);
	Quaternion operator*(const Quaternion r) const;
	Quaternion operator*(const double rv) const;
	Quaternion normalize();
};

struct Matrix3x3
{
	double arr[9];
	Matrix3x3(double t=0);
	Matrix3x3(double arr[]);
	Matrix3x3(Quaternion q);
	void set(double e,int x, int y);
	double get(int x, int y) const;
	Matrix3x3 T() const;
	Vector3d operator*(const Vector3d v) const;
	Matrix3x3 operator*(const Matrix3x3 m) const;
	Matrix3x3 operator+(const Matrix3x3 m) const;
	Matrix3x3 operator*(const double k) const;
	Matrix3x3 operator=(Matrix3x3 m);
	double det() const;
	double adj(int x, int y) const;
	static Matrix3x3 Inv(Matrix3x3 m);
	static Matrix3x3 InvDiag(Matrix3x3 m);
	static Matrix3x3 crossPMatrix(Vector3d v);
};


struct State
{
	Vector3d pos;
	Vector3d impulse;
	Quaternion q;
	Vector3d AngularM;
	Matrix3x3 R;


	State operator*(double r);
	State operator+(State r);

};
struct properties
{
	double mass;
	Matrix3x3 Inertia;
	Vector3d forcePoint;
	Vector3d forceDir;
	
};
struct RigidBody
{
	Vector3d size;
	properties prop;
	State state;
	Vector3d centerofmass;
	std::vector<Vector3d> points;
	//std::vector<double> pointmasses;
	RigidBody(Vector3d size);
	Vector3d getPointGlobalPos(Vector3d point);
	std::vector<Vector3d> touchPoint(double y);
	Matrix3x3 CalcInertia();
	Vector3d PointVel(Vector3d p);
	Vector3d PointVel2(Vector3d p);
	void step(double dt);
	void draw();
	Vector3d lowestPoint();
	void contact(double h);

};


class cl {
public:
	static void grav(State& res,const State& inp,const properties& cont);

	static RigidBody Euler(RigidBody initial, double dt);
	static RigidBody Midp(RigidBody initial, double dt);
	static RigidBody RK4(RigidBody initial, double dt);
	static void Plane();
};
