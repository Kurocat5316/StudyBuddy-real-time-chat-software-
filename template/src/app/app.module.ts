import { NgModule, ErrorHandler } from '@angular/core';
import { IonicApp, IonicModule, IonicErrorHandler } from 'ionic-angular';
import { FormsModule } from '@angular/forms';
import { MyApp } from './app.component';
import { HomePage } from '../pages/home/home';
import { RegisterPage } from "../pages/register/register"
import { PwretPage } from "../pages/pwret/pwret"
import { FriendListPage } from "../pages/friend-list/friend-list"
import { ChatViewPage } from "../pages/chat-view/chat-view"
import { SearchPage } from "../pages/search/search"
import { SearchViewPage } from "../pages/search-view/search-view"
import { ProfilePage } from "../pages/profile/profile"
import { ChangeNickNamePage} from "../pages/change-nick-name/change-nick-name"
import { ChangePasswordPage } from "../pages/change-password/change-password"
import { ChangeUnitPage } from "../pages/change-unit/change-unit"
import { AddUnitPage } from "../pages/add-unit/add-unit"


import { LocationTracker } from '../providers/location-tracker';


@NgModule({
  declarations: [
    MyApp,
    HomePage,
    RegisterPage,
    PwretPage,
    FriendListPage,
    ChatViewPage,
    SearchPage,
    ProfilePage,
    ChangeNickNamePage,
    ChangePasswordPage,
    ChangeUnitPage,
    AddUnitPage,
    SearchViewPage
  ],
  imports: [
    IonicModule.forRoot(MyApp),
    FormsModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    HomePage,
    RegisterPage,
    PwretPage,
    FriendListPage,
    ChatViewPage,
    SearchPage,
    ProfilePage,
    ChangeNickNamePage,
    ChangePasswordPage,
    ChangeUnitPage,
    AddUnitPage,
    SearchViewPage,
  ],
  providers: [{provide: ErrorHandler, useClass: IonicErrorHandler}, LocationTracker]
})
export class AppModule {}
