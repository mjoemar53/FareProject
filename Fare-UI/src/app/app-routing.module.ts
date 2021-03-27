import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CashinComponent } from './components/cashin/cashin.component';
import { MainComponent } from './components/main/main.component';
import { ManagementComponent } from './components/management/management.component';

const routes: Routes = [
  { path: '', component: MainComponent },
  { path: 'CashIn', component: CashinComponent },
  { path: 'Register', component: ManagementComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
