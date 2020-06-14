
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ThirdPartyExperienceType
{
    Tabletop = 0,
    Room = 1,
    NewScene = 2
}

[Serializable]
public class ThirdPartyExperienceData
{
    public ThirdPartyExperienceType ExperienceType;
    public string ExperienceName;
    public Texture ExperienceThumbnail;
}
