import { Component } from '@angular/core';
import { NavController, NavParams,  MenuController} from 'ionic-angular';
import { ChangeNickNamePage} from "../change-nick-name/change-nick-name"
import { ChangePasswordPage } from "../change-password/change-password"
import { ChangeUnitPage } from "../change-unit/change-unit"
/*
  Generated class for the Profile page.

  See http://ionicframework.com/docs/v2/components/#navigation for more info on
  Ionic pages and navigation.
*/
@Component({
  selector: 'page-profile',
  templateUrl: 'profile.html'
})
export class ProfilePage {

  pages: Array<{title: string, component: any}>;
  
  constructor(public navCtrl: NavController, 
  public navParams: NavParams, 
  public menu: MenuController,
  ) {
    this.menu.swipeEnable(false);
    this.pages = [
      { title: 'Edit NickName', component: ChangeNickNamePage },
      { title: 'Edit Unit', component: ChangeUnitPage },
      { title: 'Change Password', component: ChangePasswordPage },
    ];
  }

  openPage(page){
    this.navCtrl.push(page.component);
  }

  back(){
    this.menu.swipeEnable(true);
    this.navCtrl.pop();
  }

}
