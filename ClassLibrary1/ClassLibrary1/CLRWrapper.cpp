// This is the main DLL file.
 
#include "stdafx.h"
 
#include "CLRWrapper.h"
#include "CppMath.cpp"
 
CLRWrapper::CppMathWrapper::CppMathWrapper()
{
    cppMath = new CppMath();
}
 
double CLRWrapper::CppMathWrapper::Add(double arg1,double arg2)
{
    return cppMath->Add(arg1,arg2);
}
 
double CLRWrapper::CppMathWrapper::Subtract(double arg1,double arg2)
{
    return cppMath->Subtract(arg1,arg2);
}
 
double CLRWrapper::CppMathWrapper::Multiply(double arg1,double arg2)
{
    return cppMath->Multiply(arg1,arg2);
}
 
double CLRWrapper::CppMathWrapper::Divide(double arg1,double arg2)
{
    return cppMath->Divide(arg1,arg2);
}