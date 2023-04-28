using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mbs
{
    // Custom math operations used at various portions of the game. 
    public class CustomMath
    {
        // Rotates the 2D vector (around its z-axis).
        public static Vector2 Rotate(Vector2 v, float angle, bool inDegrees)
        {
            // Converts the angle to radians if provided in degrees.
            angle = (inDegrees) ? Mathf.Deg2Rad * angle : angle;

            // Calculates the rotation using matrix math...
            // Except it manually puts in what the resulting calculation would be).
            Vector2 result;
            result.x = (v.x * Mathf.Cos(angle)) - (v.y * Mathf.Sin(angle));
            result.y = (v.x * Mathf.Sin(angle)) + (v.y * Mathf.Cos(angle));

            return result;
        }

        // Does a matrix rotation using the provided axis.
        private static Vector3 Rotate(Vector3 v, float angle, char axis, bool inDegrees)
        {
            // Converts the angle to radians if provided in degrees.
            angle = (inDegrees) ? Mathf.Deg2Rad * angle : angle;

            // The rotation matrix.
            Matrix4x4 rMatrix = new Matrix4x4();

            // Checks what axis to rotate the vector3 on.
            switch (axis)
            {
                case 'x': // X-Axis
                case 'X':
                    // Rotation X Matrix
                    /*
                     * [1, 0, 0, 0]
                     * [0, cos a, -sin a, 0]
                     * [0, sin a, cos a, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
                    rMatrix.SetRow(1, new Vector4(0, Mathf.Cos(angle), -Mathf.Sin(angle), 0));
                    rMatrix.SetRow(2, new Vector4(0, Mathf.Sin(angle), Mathf.Cos(angle), 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                case 'y': // Y-Axis
                case 'Y':
                    // Rotation Y Matrix
                    /*
                     * [cos a, 0, sin a, 0]
                     * [0, 1, 0, 0]
                     * [-sin a, 0, cos a, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(Mathf.Cos(angle), 0, Mathf.Sin(angle), 0));
                    rMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
                    rMatrix.SetRow(2, new Vector4(-Mathf.Sin(angle), 0, Mathf.Cos(angle), 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                case 'z': // Z-Axis
                case 'Z':
                    // Rotation Z Matrix
                    /*
                     * [cos a, -sin a, 0, 0]
                     * [sin a, cos a, 0, 0]
                     * [0, 0, 1, 0]
                     * [0, 0, 0, 1]
                     */

                    rMatrix.SetRow(0, new Vector4(Mathf.Cos(angle), -Mathf.Sin(angle), 0, 0));
                    rMatrix.SetRow(1, new Vector4(Mathf.Sin(angle), Mathf.Cos(angle), 0, 0));
                    rMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
                    rMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
                    break;

                default: // Unknown
                    return v;
            }



            // The vector matrix.
            Matrix4x4 vMatrix = new Matrix4x4();
            vMatrix[0, 0] = v.x;
            vMatrix[1, 0] = v.y;
            vMatrix[2, 0] = v.z;
            vMatrix[3, 0] = 1;


            // The resulting matrix.
            Matrix4x4 resultMatrix = rMatrix * vMatrix;

            // Gets the vector3 from the result matrix.
            Vector3 resultVector = new Vector3(
                resultMatrix[0, 0],
                resultMatrix[1, 0],
                resultMatrix[2, 0]
                );

            // Returns the result.
            return resultVector;
        }

        // Rotate around the x-axis.
        public static Vector3 RotateX(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'X', inDegrees);
        }

        // Rotate around the y-axis.
        public static Vector3 RotateY(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'Y', inDegrees);
        }

        // Rotate around the z-axis.
        public static Vector3 RotateZ(Vector3 v, float angle, bool inDegrees)
        {
            return Rotate(v, angle, 'Z', inDegrees);
        }



        // INTERPOLATION

        // Catmull Rom - goes between points 1 and 2 using points 0 and 3 to create a curve.
        public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float u)
        {
            // the catmull-rom matrix, which has a 0.5F scalar applied from the start.
            Matrix4x4 matCatmullRom = new Matrix4x4();

            // setting the rows
            matCatmullRom.SetRow(0, new Vector4(0.5F * -1.0F, 0.5F * 3.0F, 0.5F * -3.0F, 0.5F * 1.0F));
            matCatmullRom.SetRow(1, new Vector4(0.5F * 2.0F, 0.5F * -5.0F, 0.5F * 4.0F, 0.5F * -1.0F));
            matCatmullRom.SetRow(2, new Vector4(0.5F * -1.0F, 0.5F * 0.0F, 0.5F * 1.0F, 0.5F * 0.0F));
            matCatmullRom.SetRow(3, new Vector4(0.5F * 0.0F, 0.5F * 2.0F, 0.5F * 0.0F, 0.5F * 0.0F));


            // Points
            Matrix4x4 pointsMat = new Matrix4x4();

            pointsMat.SetRow(0, new Vector4(p0.x, p0.y, p0.z, 0));
            pointsMat.SetRow(1, new Vector4(p1.x, p1.y, p1.z, 0));
            pointsMat.SetRow(2, new Vector4(p2.x, p2.y, p2.z, 0));
            pointsMat.SetRow(3, new Vector4(p3.x, p3.y, p3.z, 0));


            // Matrix for u to the power of given functions.
            Matrix4x4 uMat = new Matrix4x4(); // the matrix for 'u' (also called 't').

            // Setting the 'u' values to the proper row, since this is being used as a 1 X 4 matrix.
            uMat.SetRow(0, new Vector4(Mathf.Pow(u, 3), Mathf.Pow(u, 2), Mathf.Pow(u, 1), Mathf.Pow(u, 0)));

            // Result matrix from a calculation. 
            Matrix4x4 result;

            // Order of [u^3, u^2, u, 0] * M * <points matrix>
            // The catmull-rom matrix has already had the (1/2) scalar applied.
            result = matCatmullRom * pointsMat;

            result = uMat * result; // [u^3, u^2, u, 0] * (M * points)

            // the resulting values are stored at the top.
            return result.GetRow(0);
        }

        // Bezier - interpolates between two points using 2 control points to change the movement curve.
        public static Vector3 Bezier(Vector3 t1, Vector3 p1, Vector3 p2, Vector3 t2, float u)
        {
            // Bezier matrix
            Matrix4x4 matBezier = new Matrix4x4();

            matBezier.SetRow(0, new Vector4(-1, 3, -3, 1));
            matBezier.SetRow(1, new Vector4(3, -6, 3, 0));
            matBezier.SetRow(2, new Vector4(-3, 3, 0, 0));
            matBezier.SetRow(3, new Vector4(1, 0, 0, 0));


            // Result matrix from a calculation. 
            Matrix4x4 result;

            // The two points on the line, and their control points
            Matrix4x4 pointsMat = new Matrix4x4();

            pointsMat.SetRow(0, new Vector4(p1.x, p1.y, p1.z, 0));
            pointsMat.SetRow(1, new Vector4(t1.x, t1.y, t1.z, 0));
            pointsMat.SetRow(2, new Vector4(t2.x, t2.y, t2.z, 0));
            pointsMat.SetRow(3, new Vector4(p2.x, p2.y, p2.z, 0));


            // Matrix for 'u' to the exponent 0 through 3.
            Matrix4x4 uMat = new Matrix4x4(); // the matrix for 'u' (also called 't').

            // Setting the 'u' values to the proper row, since this is being used as a 1 X 4 matrix.
            // The exponent values are being applied as well.
            uMat.SetRow(0, new Vector4(Mathf.Pow(u, 3), Mathf.Pow(u, 2), Mathf.Pow(u, 1), Mathf.Pow(u, 0)));

            // Doing the bezier calculation
            // Order of [u^3, u^2, u, 0] * M * <points matrix>
            result = matBezier * pointsMat; // bezier matrix * points matrix
            result = uMat * result; // u matrix * (bezier matrix * points matrix)

            // the needed values are stored at the top of the result matrix.
            return result.GetRow(0);
        }
    }
}