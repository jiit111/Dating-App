import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-cards',
  templateUrl: './member-cards.component.html',
  styleUrls: ['./member-cards.component.css']
})
export class MemberCardsComponent implements OnInit {
  @Input() member: Member;
  constructor() { }

  ngOnInit(): void {
  }

}
