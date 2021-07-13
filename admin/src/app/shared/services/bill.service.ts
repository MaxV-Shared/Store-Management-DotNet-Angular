import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Bill, BillCreateRequest, ENVIRONMENT } from '@app/models';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { BaseService } from './base/base.service';
import { IBaseService } from './interface';

@Injectable({
    providedIn: 'root'
})
export class BillService extends BaseService implements IBaseService<BillCreateRequest,Bill> {

    constructor(http: HttpClient,
        public translate: TranslateService) {
        super(http);
    }
    add(entity: BillCreateRequest): Observable<any> {
        return this.http.post(`${ENVIRONMENT.apiUrl}/api/bills`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe();
    }
    update(id: number, entity: Bill): Observable<any> {
        return this.http.put(`${ENVIRONMENT.apiUrl}/api/bills/${id}`, JSON.stringify(entity), { headers: this._sharedHeaders })
            // .pipe(catchError(this.handleError));
    }
    getPaging(pageIndex: number, pageSize: number, searchText: string): Observable<Object> {
        return this.http.get(`${ENVIRONMENT.apiUrl}/api/bills/filter?pageIndex=${pageIndex}&pageSize=${pageSize}&searchText=${searchText}&langId=${this.translate.currentLang}`, { headers: this._sharedHeaders })
            // .pipe(catchError(this.handleError));
    }
    getAll(): Observable<any> {
        return this.http.get(`${ENVIRONMENT.apiUrl}/api/bills`, { headers: this._sharedHeaders })
            // .pipe(catchError(this.handleError));
    }
    getDetails(billId: number): Observable<any> {
        return this.http.get(`${ENVIRONMENT.apiUrl}/api/bills/${billId}/details?langId=${this.translate.currentLang}`, { headers: this._sharedHeaders })
            // .pipe(catchError(this.handleError));
    }

}
