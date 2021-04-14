using System;

[Serializable]
public class Cost
{
    public Resource[] resources;
    public Cost(Resource[] resources)
    {
        this.resources = resources;
    }


}
