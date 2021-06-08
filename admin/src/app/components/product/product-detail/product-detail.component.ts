import { Component, EventEmitter, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import CategoryDetailComponent from '@app/components/category/category-detail/category-detail.component';
import { TranslateService } from '@ngx-translate/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Lang, Category, Product, ProductDetail, environment, langs, CategoryDetail } from '@app/models';
import { CategoryService, UtilitiesService } from '@app/shared/services';
import { ProductService } from '@app/shared/services/product.service';

@Component({
    selector: 'app-product-detail',
    templateUrl: './product-detail.component.html',
    styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {

    constructor(private modalService: BsModalService,
        private categoryService: CategoryService,
        private productService: ProductService,
        private utilitiesService: UtilitiesService,
        public bsModalRef: BsModalRef,
        public bsCategoryModalRef: BsModalRef,
        public translate: TranslateService) {

    }
    showAddCategory = false;
    showEditCategory = false;
    langs: Lang[];
    public productId: number;
    saved: EventEmitter<any> = new EventEmitter();
    entity: Product;
    myControl = new FormControl();
    options: string[] = ['One', 'Two', 'Three'];
    filteredOptions: Observable<CategoryDetail[]>;
    categories: CategoryDetail[];
    uploadedFiles: any[] = [];
    private subscription = new Subscription();
    getEntity() {
        this.entity = {
            categoryId: 1,
            id: 1,
            price: 100000,
            urlImage: "",
            details: [{
                langId: "vi",
                name: "Sản phẩm 1",
                description: "Mô tả về sản phẩm: Lorem Ipsum chỉ đơn giản là một đoạn văn bản giả, ",
                product: null,
                productId: 1
            }, {
                langId: "en",
                name: "Product 1",
                description: "Product description: Lorem ipsum dolor sit amet, consectetur adipiscing elit,",
                product: null,
                productId: 1
            }
            ]
        };

        // this.categories = [
        //     { categoryId: 1, name: "Danh mục 1", description: "11", category: { id: 1, parentId: null }, langId: "vi" },
        //     { categoryId: 2, name: "Danh mục 2", description: "22", category: { id: 2, parentId: null }, langId: "vi" },
        // ];


    }
    ngOnInit(): void {
        console.log(this.productId);
        this.langs = langs;
        this.getEntity();
        this.getCategory();
    }

    onSave() {
        // this.saved.emit("Saved");
        //console.log(this.entity);

        const formData = this.utilitiesService.ToFormData(this.entity);
        formData.append('file', this.uploadedFiles[0], this.uploadedFiles[0].name);
        this.productService.add(formData).subscribe(() => {
            //this.log
        });
    }
    changeTab(index: number) {
        // console.log(this.langs[index].id);
        // this.translate.use(this.langs[index].id);
    }
    log(item: any) {
        console.log(item)
    }
    displayFn(user: CategoryDetail): string {
        return user && user.name ? user.name : '';
    }

    private _filter(name: string): CategoryDetail[] {
        const filterValue = name.toLowerCase();
        return this.categories.filter(option => option.name.toLowerCase().indexOf(filterValue) >= 0);
    }
    dealWithFiles(event) {
        this.uploadedFiles = event.currentFiles;
    }
    createCategory() {
        const initialState = {
            id: null,
        };

        this.bsCategoryModalRef = this.modalService.show(CategoryDetailComponent, {
            initialState: initialState,
            class: 'modal-lg',
            backdrop: 'static'
        });

        this.bsCategoryModalRef.content.saved.subscribe((e) => {
            this.bsCategoryModalRef.hide();
            this.getCategory();
        });
    }

    editCategory(categoryId) {
        const initialState = {
            id: categoryId,
        };

        this.bsCategoryModalRef = this.modalService.show(CategoryDetailComponent, {
            initialState: initialState,
            class: 'modal-lg',
            backdrop: 'static',
            focus: false
        });

        this.bsCategoryModalRef.content.saved.subscribe((e) => {
            this.bsCategoryModalRef.hide();
            this.getCategory();
        });
    }
    getCategory() {
        this.log(this.translate.currentLang);
        this.subscription.add(this.categoryService.getAll(this.translate.currentLang).subscribe((res: CategoryDetail[]) => {
            this.categories = res;
            this.filteredOptions = this.myControl.valueChanges
                .pipe(
                    startWith(''),
                    map(value => typeof value === 'string' ? value : value.name),
                    map(name => name ? this._filter(name) : this.categories.slice())
                );
        }))
    }
}
