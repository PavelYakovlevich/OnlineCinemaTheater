import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatFormFieldModule } from '@angular/material/form-field'
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input'
import { MatButtonModule } from '@angular/material/button'
import { MatToolbarModule } from '@angular/material/toolbar'
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { ErrorAlertComponent } from './components/error-alert/error-alert.component';
import { RegistrationFormComponent } from './components/registration-form/registration-form.component';
import { HomeComponent } from './components/home/home.component';
import { MediaListComponent } from './components/media-list/media-list.component';
import { MediaCardComponent } from './components/media-card/media-card.component';
import { MediaFilterComponent } from './components/media-filter/media-filter.component';
import { WelcomePageComponent } from './components/welcome-page/welcome-page.component';
import { MediaComponent } from './components/media/media.component';
import { ParticipantComponent } from './components/participant/participant.component';
import { CommentBoxComponent } from './components/comment-box/comment-box.component';
import { AuthInterceptor } from './services/infrastructure/auth-interceptor';
import { ErrorBoxComponent } from './components/error-box/error-box.component';
import { CommentsListComponent } from './components/comments-list/comments-list.component';
import { MediaInteractDialogComponent } from './components/media-interact-dialog/media-interact-dialog.component';
import { MatNativeDateModule } from '@angular/material/core';
import { CountrySelectComponent } from './components/country-select/country-select.component';
import { GenreSelectComponent } from './components/genre-select/genre-select.component';
import { ParticipantsListComponent } from './components/participants-list/participants-list.component';
import { PictureContainerComponent } from './components/picture-container/picture-container.component';
import { ParticipantInteractDialogComponent } from './components/participant-interact-dialog/participant-interact-dialog.component';
import { SearchParticipantComponent } from './components/search-participant/search-participant.component';
import { GenreManipulationListComponent } from './components/genre-manipulation-list/genre-manipulation-list.component';
import { UserPageComponent } from './components/user-page/user-page.component';
import { EmailVerifiedPageComponent } from './components/email-verified-page/email-verified-page.component';
import { ChangePasswordPageComponent } from './components/change-password-page/change-password-page.component';
import { ForgotPasswordPageComponent } from './components/forgot-password-page/forgot-password-page.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginFormComponent,
    ErrorAlertComponent,
    RegistrationFormComponent,
    HomeComponent,
    MediaListComponent,
    MediaCardComponent,
    MediaFilterComponent,
    WelcomePageComponent,
    MediaComponent,
    ParticipantComponent,
    CommentBoxComponent,
    ErrorBoxComponent,
    CommentsListComponent,
    MediaInteractDialogComponent,
    CountrySelectComponent,
    GenreSelectComponent,
    ParticipantsListComponent,
    PictureContainerComponent,
    ParticipantInteractDialogComponent,
    SearchParticipantComponent,
    GenreManipulationListComponent,
    UserPageComponent,
    EmailVerifiedPageComponent,
    ChangePasswordPageComponent,
    ForgotPasswordPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatDividerModule,
    MatChipsModule,
    InfiniteScrollModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    MatSidenavModule,
    MatExpansionModule,
    MatTabsModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatStepperModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
