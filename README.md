# untiy-nurbs

Unity base NURBS spline and surface module

## Spline
Spline module offers **Standard, Loop, Clamped** mode. 

`GetCurve` function offers [0, 1) -> position on curve in whichever mode. First & second devivatives are available from `GetFirstDerivative` and `GetSecondDerivative` as they are shown below, currently only active Standard or Loop mode.

<img src="Imgs/dv_1.png" width="50%"><img src="Imgs/dv_2.png" width="50%">

## Surface

You can create NURBS surfaced mesh inside Unity.
![img](Imgs/Capture.PNG)

Move Control Points and Bake out as a mesh. 
![img](Imgs/output.gif)

## Reference
[Shape Interrogation for Computer Aided Design and Manufacturing, Nicholas M. PatrikalakisTakashi Maekawa, Splinger, 2002](https://link.springer.com/book/10.1007/978-3-642-04074-0)