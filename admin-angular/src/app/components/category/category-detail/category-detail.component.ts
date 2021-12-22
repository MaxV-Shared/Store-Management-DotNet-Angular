import { Component, EventEmitter, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Lang, Category, LANGS, CategoryDetail, CategoryCreateRequest, CategoryUpdateRequest, mapper, BaseUpdateRequest } from '@app/models';
import { CategoryService, UtilitiesService } from '@app/shared/services';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { HttpResponse } from '@angular/common/http';
import { BaseComponent } from '@app/components/base';
import { CategoryDetailUpdateRequest } from '../../../models/update-requests/category-detail-update-request';
@Component({
    selector: 'app-category-detail',
    templateUrl: './category-detail.component.html',
    styleUrls: ['./category-detail.component.scss']
})
export default class CategoryDetailComponent extends BaseComponent implements OnInit {

    constructor(private modalService: NgbModal,
        public bsModalRef: BsModalRef,
        private categoryService: CategoryService,
        public toastr: ToastrService,
        private utilitiesService: UtilitiesService,
        public translate: TranslateService) {
        super(translate, toastr);
    }
    langs: Lang[];
    public id: number;
    saved: EventEmitter<any> = new EventEmitter();
    public entity: Category;
    private subscription = new Subscription();
    a2: CategoryUpdateRequest;
    ngOnInit() {
        // automapper
        this.langs = LANGS;

        this.entity = {
            details: [
                {
                    langId: 'vi',
                    name: '',
                    description: '',

                },
                {
                    langId: 'en',
                    name: '',
                    description: '',
                }
            ]
        }
        if (this.id != null) {
            this.subscription.add(this.categoryService.getById(this.id).subscribe((res: HttpResponse<Category>) => {
                if (res.status == 200) {
                    this.entity = res.body;
                }
            }, err => {
                console.error(err);
                this.notifyError("Error");
            }));
        }
    }

    onSave() {
        if (this.id == null) {
            this.add();
        } else {
            this.update(this.id);
        }
    }

    add() {
        let objCreate = mapper.map(this.entity, CategoryCreateRequest, Category);
        this.subscription.add(this.categoryService.add(objCreate)
            .subscribe(() => {
                this.notifySuccess('Success');
                this.saved.emit("success");
            }, err => {
                console.error(err)
            }));
    }

    update(id: number) {
        let objUpdate : any = mapper.map(this.entity, CategoryUpdateRequest, Category);
        let formData : any = this.utilitiesService.ToFormData(objUpdate);
        console.log(objUpdate instanceof BaseUpdateRequest);
        console.log(formData instanceof FormData);

        this.subscription.add(this.categoryService.update(id, objUpdate)
            .subscribe(() => {
                this.notifySuccess('Success');
                this.saved.emit("success");
            }, err => {
                console.error(err)
            }));
    }

    changeTab(index: number) {
        // console.log(this.langs[index].id);
        // this.translate.use(this.langs[index].id);
    }
}
