#pragma once
 
#include "CppMath.h"
 
using namespace System;
 
namespace CLRWrapper {
    public ref class CppMathWrapper
    {
    public:
        //ctor
        CppMathWrapper();
        //Methods
        double Add(double arg1,double arg2);
    double Subtract(double arg1, double arg2);
    double Multiply(double arg1,double arg2);
    double Divide (double arg1,double arg2);
    private:
        CppMath *cppMath;
    };
}