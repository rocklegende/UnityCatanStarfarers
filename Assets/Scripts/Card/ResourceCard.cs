using System;

[Serializable]
public abstract class ResourceCard : Card
{
    public Resource resource;

    public ResourceCard(Resource resource) : base()
    {
        this.resource = resource;
    }
}
