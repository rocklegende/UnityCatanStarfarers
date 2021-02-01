using System;
public class BuildDropDownOption
{
    public delegate void DelegateMethod(Token token);
    public string imageName;
    public Token token;
    public DelegateMethod method;
    public bool disabled = false;

    public BuildDropDownOption(string imageName, Token token, DelegateMethod method)
    {
        this.imageName = imageName;
        this.token = token;
        this.method = method;
    }

}
