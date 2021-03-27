import { Component, OnInit, Input } from '@angular/core';
import { throwError} from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiService } from 'src/app/services';

function amountChecker(c: AbstractControl) {
  return c.get('amount')!.value > c.get('cashAmount')!.value ? { 'invalidAmounts': {message: "The Amount cannot be greater than the Cash Amount"} } : null;
}

@Component({
  selector: 'app-cashin',
  templateUrl: './cashin.component.html',
  styleUrls: ['./cashin.component.less']
})
export class CashinComponent implements OnInit {
  cashInForm: FormGroup;

  //1: Success 2: Error 3: Warning
  alertState: number = 0;
  alertVisiblity: boolean = false;
  @Input() alertMessage: string = '';

  constructor(private formBuilder: FormBuilder, private apiService: ApiService) { 
    this.cashInForm = formBuilder.group({
      cardId: ['', { validators: [Validators.required] }],
      amount: ['', { validators: [Validators.required, Validators.min(100), Validators.max(10000)] }],
      cashAmount: ['', {validators: [Validators.required] }]
    }, { validators: amountChecker });
  }

  ngOnInit(): void {
  }

  onSubmit() {
    this.alertVisiblity = false;
    const result = Object.assign({},this.cashInForm.value);
    this.apiService.topUp(result.cardId, result.amount, result.cashAmount)
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
        this.alertMessage = `New Balance: ${data.newBalance} - Change: ${data.change}`
        this.alertVisiblity = true;
        this.resetForm();
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
    this.cashInForm.reset();
  }
}
