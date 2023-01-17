import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChangePasswordPageComponent } from './components/change-password-page/change-password-page.component';
import { EmailVerifiedPageComponent } from './components/email-verified-page/email-verified-page.component';
import { ForgotPasswordPageComponent } from './components/forgot-password-page/forgot-password-page.component';
import { HomeComponent } from './components/home/home.component';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { MediaListComponent } from './components/media-list/media-list.component';
import { MediaComponent } from './components/media/media.component';
import { ParticipantComponent } from './components/participant/participant.component';
import { RegistrationFormComponent } from './components/registration-form/registration-form.component';
import { UserPageComponent } from './components/user-page/user-page.component';
import { WelcomePageComponent } from './components/welcome-page/welcome-page.component';

const routes: Routes = [
  { path: "login", component: LoginFormComponent },
  { path: "registration", component: RegistrationFormComponent },
  { path: 'welcome', component: WelcomePageComponent },
  { path: 'confirm-mail', component: EmailVerifiedPageComponent },
  { path: 'forgot-password', component: ForgotPasswordPageComponent },
  { path: ':userId/change-password', component: ChangePasswordPageComponent },
  { path: '', component: HomeComponent, children: 
    [
      {
        path: 'media/:mediaId', 
        component: MediaComponent
      },
      {
        path: 'participant/:participantId', 
        component: ParticipantComponent
      },
      {
        path: 'user', 
        component: UserPageComponent
      },
      {
        path: '**', 
        component: MediaListComponent
      }
    ] 
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
