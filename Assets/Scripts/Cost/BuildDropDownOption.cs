using System;
public class BuildDropDownOption
{
    public BuildableToken buildableToken; 
    public string imageName;
    public Cost cost;
    public bool disabled = false;

    public BuildDropDownOption(BuildableToken buildableToken, string imageName, Cost cost)
    {
        this.imageName = imageName;
        this.cost = cost;
        this.buildableToken = buildableToken;
    }

}
