using Godot;
using System;

public partial class MainMenu : Node2D
{

    MenuButton menuButton;
    public override void _Ready()
    {
       
        menuButton = GetNode<MenuButton>("./PlayMenu");
    }

    public override void _Process(double delta)
    {
        menuButton.GetPopup().IdPressed += MainMenu_IdPressed;
            

    }

    private void MainMenu_IdPressed(long id)
    {
        if(id==1)
        {
            //gamemanager start solos
        }
        else
        {
            //coop
        }
        return;
    }
}