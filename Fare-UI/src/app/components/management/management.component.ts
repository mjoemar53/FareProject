import { Component, OnInit, Input } from '@angular/core';
import { throwError} from 'rxjs';
import { catchError } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from 'src/app/services';

@Component({
  selector: 'app-management',
  templateUrl: './management.component.html',
  styleUrls: ['./management.component.less']
})
export class ManagementComponent implements OnInit {
  registerForm: FormGroup;

  //1: Success 2: Error 3: Warning
  alertState: number = 0;
  alertVisiblity: boolean = false;
  @Input() alertMessage: string = '';

  constructor(private formBuilder: FormBuilder, private apiService: ApiService) { 
    this.registerForm = formBuilder.group({
      cardId: ['', { validators: [Validators.required] }],
      registeredId: ['', {validators: [Validators.required, Validators.pattern('^(.{2,4})-(.{4})-(.{4})')] }]
    });
  }

  ngOnInit(): void {
  }

  onSubmit() {
    this.alertVisiblity = false;
    const result = Object.assign({},this.registerForm.value);
    this.apiService.registerCard(result.cardId, result.registeredId)
    .pipe(
      catchError(err => {
        if(!!err.error.errorMessage) {
          return throwError(err);
        }
        // handle client side error
        err.error.errorMessage = `${err.statusText} has occurred - Please contact administrator. `
        return throwError(err);
      })
    )
    .subscribe(
      data => {
        this.alertState = 1;
        this.alertMessage = data.message;
        this.alertVisiblity = true;
        setTimeout(() => {
          this.resetForm();
        }, 3000);
      },
      err => {
        this.alertState = 2;
        this.alertMessage = err.error.errorMessage;
        this.alertVisiblity = true;
      }
    )
  }

  private resetForm(): void {
    this.alertVisiblity = false;
    this.alertState = 0;
    this.registerForm.reset();
  }

}
