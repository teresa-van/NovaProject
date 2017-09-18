using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[DataContract]
public class ObjectModel
{

    #region Constructors

    public ObjectModel()
        : this("", "", 0, 0, 0, 0, 0, 0, 0, 0, 0)
    {

    }

    public ObjectModel(string name, string parent, 
        float positionX, float positionY, float positionZ,
        float rotationX, float rotationY, float rotationZ,
        float scaleX, float scaleY, float scaleZ)
    {
        this.Name = name;
        this.Parent = parent;

        this.PositionX = positionX;
        this.PositionY = positionY;
        this.PositionZ = positionZ;

        this.RotationX = rotationX;
        this.RotationY = rotationY;
        this.RotationZ = rotationZ;

        this.ScaleX = scaleX;
        this.ScaleY = scaleY;
        this.ScaleZ = scaleZ;

    }

    #endregion

    #region Properties
    [DataMember]
    public string Name { set; get; }

    [DataMember]
    public string Parent { set; get; }

    [DataMember]
    public float PositionX { set; get; }
    [DataMember]
    public float PositionY { set; get; }
    [DataMember]
    public float PositionZ { set; get; }

    [DataMember]
    public float RotationX { set; get; }
    [DataMember]
    public float RotationY { set; get; }
    [DataMember]
    public float RotationZ { set; get; }

    [DataMember]
    public float ScaleX { set; get; }
    [DataMember]
    public float ScaleY { set; get; }
    [DataMember]
    public float ScaleZ { set; get; }

    #endregion

}
