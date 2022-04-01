# untiy-nurbs

Unity base NURBS spline and surface module. Both Spline and Surface are supported.

## Spline
Spline module offers **Standard, Loop, Clamped** mode. 

`GetCurve` function converts [0, 1) -> position on curve in whichever mode. First & second devivatives are available from `GetFirstDerivative` and `GetSecondDerivative` as they are shown below, currently only active Standard or Loop mode. 

<img src="Imgs/dv_1.png" width="50%"><img src="Imgs/dv_2.png" width="50%">

## Surface

You can create open/closed surfaced mesh. Same as Spline, it offers **Standard, Loop, Clamped** mode for each direction (because B-Spline surface is a Tensor of B-Spline). This means it supports **FlatSurface, Torus(S x S), Slinder** right now.

By moving control points using Unity built-in Handle UI you can create mesh in runtime or not. Besides you can Bake out as a mesh with `CreateOrUpdate` method in HandlerEditor. 

![img](Imgs/torus.png)

## TODO
- [ ] weight is not considered in derivatives of spline.
- [ ] Sphere (S^2) option in surface.

## Reference
[Shape Interrogation for Computer Aided Design and Manufacturing, Nicholas M. PatrikalakisTakashi Maekawa, Splinger, 2002](https://link.springer.com/book/10.1007/978-3-642-04074-0)