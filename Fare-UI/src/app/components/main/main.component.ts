import { Component, Input, OnInit } from '@angular/core';
import { Observable, of, throwError} from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService } from 'src/app/services';


@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.less']
})
export class MainComponent implements OnInit {
  cardId: string = '';

  //1: Success 2: Error 3: Warning
  alertState: number = 0;
  alertVisiblity: boolean = false;
  @Input() alertMessage: string = '';

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
  }

  chargeCard() {
    this.alertState = 0;
    this.alertVisiblity = false;

    if(this.cardId === "") {
      this.alertState = 3;
      this.alertMessage = "Please scan/enter card Id.";
      this.alertVisiblity = true;
      return;
    }
    //Add some validations here

    this.apiService.chargeFare(this.cardId)
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
      },
      err => {
        this.alertState = 2;
        this.alertMessage = err.error.errorMessage;
        this.alertVisiblity = true;
      }
    )
  }


}
