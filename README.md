# untiy-nurbs

NURBS spline and surface module for Unity. Both spline/surface are modifiable in runtime.

### Spline

Spline module offers **Standard, Loop, Clamped** options. 

`GetCurve` function converts [0, 1) -> position on curve. First & second devivatives are available from `GetFirstDerivative` and `GetSecondDerivative`, are currently active only in Standard or Loop mode. (both first and second derivatives are shown below)

<img src="Imgs/spline_1.png" width="50%"/><img src="Imgs/spline_2.png" width="50%"/>



### Surface

Same as Spline, this module offers **Standard, Loop, Clamped** options for each direction (because NURBS surface is a Tensor of NURBS spline), which means this supports **Plane, Cylinder, Torus, Sphere (Clamped top and bottom)**.

<img src="Imgs/torus_sample.png" width="25%"/><img src="Imgs/sphere_sample.png" width="25%"/><img src="Imgs/plane_sample.png" width="25%"/><img src="Imgs/cylinder_sample.png" width="25%"/>

By moving control points using unity built-in handle UI, you can modify a mesh in runtime. Once you finish modifying you can **bake the mesh out and save as an asset** with `CreateOrUpdate` method.

<img src="Imgs/output.gif" width="100%"/>



### Common
As seen in the Demo all the data is stored in ScriptableObject. Other settings are very obvious but if you have some trouble or question feel free to make a issue.

### TODO
- [ ] Weight support in derivatives of spline.
- [ ] Derivatives in clamped spline.

## Reference
[Shape Interrogation for Computer Aided Design and Manufacturing, Nicholas M. PatrikalakisTakashi Maekawa, Splinger, 2002](https://link.springer.com/book/10.1007/978-3-642-04074-0)

[Introduction to Computing with Geometry Notes, Department of Computer Science, Michigan Technological University, 1997-2014 C.-K. Shene](https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/)