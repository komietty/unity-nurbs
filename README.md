# untiy-nurbs

NURBS spline and surface module for Unity. Both spline/surface are modifiable in the runtime, and all the data is automatically serialized in ScriptableObject.

### Spline

The spline module has **Standard**, **Loop**, and **Clamped** options. The `GetCurve` function converts [0, 1) to a position on a curve. First & second devivatives are available with methods named `GetFirstDerivative` and `GetSecondDerivative`, which are currently avairavle in Standard or Loop mode. 

<div display="flex" justify-content="space">
    <img src="/Imgs/spline_1.png" width="412"/>
    <img src="/Imgs/spline_2.png" width="412"/>
</div>

### Surface

Same as Spline, the NURBS surface module has **Standard**, **Loop**, and **Clamped** options for each direction (because NURBS surface is a Tensor of NURBS spline). This means the surface module supports **Plane, Cylinder, Torus, Sphere (Clamped top and bottom)**. By moving control points using unity built-in handle UI, you can modify a mesh in runtime. Once you finish modifying you can **bake the mesh out and save as an asset** with `CreateOrUpdate` method.

<img src="/Imgs/output.gif" width="100%"/>


### TODO
- [ ] Weight support in derivatives of spline.
- [ ] Derivatives in clamped spline.

## Reference
- [Shape Interrogation for Computer Aided Design and Manufacturing, Patrikalakis & Maekawa, Splinger, 2002](https://link.springer.com/book/10.1007/978-3-642-04074-0)

- [Introduction to Computing with Geometry Notes, Department of Computer Science, Michigan Technological University, 1997-2014 C.-K. Shene](https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/)