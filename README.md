# untiy-nurbs

Unity base NURBS spline and surface module. Both Spline and Surface are available.

## Spline
Spline module offers **Standard, Loop, Clamped** options. 

`GetCurve` function converts [0, 1) -> position on curve in whichever mode. First & second devivatives are available from `GetFirstDerivative` and `GetSecondDerivative` as they are shown below, are currently active only in Standard or Loop mode. 

<img src="Imgs/dv_1.png" width="50%"><img src="Imgs/dv_2.png" width="50%">

## Surface

Open/Closed surface mesh is available. Same as Spline, this module offers **Standard, Loop, Clamped** options for each direction (because NURBS surface is a Tensor of NURBS spline), which means this supports **FlatSurface, Torus(S x S), Cylinder**.

By moving control points using unity built-in handle UI, you can modify a mesh in runtime. Once you finish modifying you can **bake the mesh out and save as an asset** with `CreateOrUpdate` method.

![img](Imgs/torus.png)

## Common
As seen in the Demo all the data is stored in ScriptableObject. Other settings are very obvious but if you have some trouble or question feel free to make a issue.

## TODO
- [ ] Weight support in derivatives of spline.
- [ ] Derivatives in clamped spline.
- [ ] Sphere (S^2) option in surface.
- [ ] Enable FFD.

## Reference
[Shape Interrogation for Computer Aided Design and Manufacturing, Nicholas M. PatrikalakisTakashi Maekawa, Splinger, 2002](https://link.springer.com/book/10.1007/978-3-642-04074-0)

[Introduction to Computing with Geometry Notes, Department of Computer Science, Michigan Technological University, 1997-2014 C.-K. Shene](https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/)