import { ICategoryAddOrUpdateRequest, ICategoryDetailViewModel } from 'models';
import { ToFormData } from 'utils';
import { IBasePaging, ICategoryViewModel } from '../models';
import { IFilterBodyRequest } from '../models/Bases/IFilterBodyRequest';
import axiosClient from './axiosClient';

const categoryApi = {
  getAll(data: IFilterBodyRequest): Promise<IBasePaging<ICategoryDetailViewModel>> {
    const url = '/categories/filter';
    return axiosClient().post(url, data);
  },
  getById(id: number): Promise<ICategoryViewModel> {
    const url = `/categories/${id}`;
    return axiosClient().get(url);
  },
  // add(data: ICategoryAddOrUpdateRequest): Promise<any> {
  //   const url = '/categories';
  //   return axiosClient().post(url, data);
  // },
  // update(data: ICategoryAddOrUpdateRequest): Promise<any> {
  //   const url = `/categories/${data.id}`;
  //   return axiosClient().put(url, data);
  // },
  addOrUpdate(request: ICategoryAddOrUpdateRequest): Promise<any> {
    console.log("categoryApi.addOrUpdate");
    var formRequest = new FormData();
    formRequest = ToFormData(request, formRequest);
    if (request.id == null) {
      const url = '/categories';
      return axiosClient().post(url, formRequest);
    }
    const url = `/categories/${request.id}`;
    return axiosClient().put(url, formRequest);
  },
  remove(id: string): Promise<any> {
    const url = `/categories/${id}`;
    return axiosClient().delete(url);
  },
};

export default categoryApi;