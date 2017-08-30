import { Component, ViewChild } from '@angular/core';
import { Nav, Platform, LoadingController, ToastController } from 'ionic-angular';
import { StatusBar, Splashscreen } from 'ionic-native';
import {Http,Headers} from '@angular/http';

import { HomePage } from '../pages/home/home';

import { SearchPage} from '../pages/search/search'
import { ProfilePage } from '../pages/profile/profile'

import { ShareData } from "../pages/sharedata/sharedata"
//import { FriendListPage } from "../pages/friend-list/friend-list"

@Component({
  templateUrl: 'app.html',
  providers: [ShareData]
})
export class MyApp {
  @ViewChild(Nav) nav: Nav;

  rootPage = HomePage;

  pages: Array<{title: string, component: any}>;
  

  constructor(platform: Platform,
  public loadingCtrl: LoadingController,
  public sharedata: ShareData,
  public http: Http,
  public toastCtrl: ToastController) {
    platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      StatusBar.styleDefault();
      Splashscreen.hide();
    });


    // used for an example of ngFor and navigation
    this.pages = [
      { title: 'Search', component: SearchPage },
      { title: 'Profile', component: ProfilePage },
    ];
    
  }

    openPage(page) {
    this.nav.push(page.component);
  }

  LogOut(){
     let loader = this.loadingCtrl.create({
      content: "Please wait...",
    });
    loader.present();

    var link = this.sharedata.geturl() + 'Login/GetSystemList';
    var data = JSON.stringify({
      id: this.sharedata.getId(),
    });
    var headers = new Headers();
    headers.append('Content-Type', 'application/json');
        this.http.post(link, data, { headers: headers })
        .subscribe(data => {
          loader.dismiss();
          this.sharedata.setLogOut1(1);
          this.sharedata.setLogOut2(1);
          this.sharedata.setLogOut3(1);
          this.nav.setRoot(HomePage);
        }, error => {
          loader.dismiss();
            let toast = this.toastCtrl.create({
                message: 'Ooop, connection seem unstable.',
                duration: 3000
              });
            toast.present();
    });
  }
}
