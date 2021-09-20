import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  RegisterMode : boolean = false;
  users : any;
  constructor(private http:HttpClient ) { }

  ngOnInit(): void {
  
  }

  RegisterToggle(){
    this.RegisterMode = !this.RegisterMode;
  }

  cancelRegisterMode(event : boolean){
     this.RegisterMode = event;
  }

}
