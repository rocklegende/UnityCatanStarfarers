using System;
public class BuildDropDownOption
{

    public delegate void DelegateMethod();
    public string imageName;
    public Cost cost;
    public DelegateMethod method;

    public BuildDropDownOption(string imageName, Cost cost, DelegateMethod method)
    {
        this.imageName = imageName;
        this.cost = cost;
        this.method = method;
    }




    


}
