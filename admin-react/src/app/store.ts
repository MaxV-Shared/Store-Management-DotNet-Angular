import createSagaMiddleware from "@redux-saga/core";
import {
  Action,
  combineReducers,
  configureStore,
  ThunkAction,
} from "@reduxjs/toolkit";
import { connectRouter, routerMiddleware } from "connected-react-router";
import categoryReducer from "features/Category/categorySlice";
import cityReducer from "features/city/citySlice";
import dashboardReducer from "features/dashboard/dashboardSlice";
import functionReducer from "features/Function/functionSlice";
// import studentReducer from 'features/Student/studentSlice';
import authReducer from "../features/auth/authSlice";
import { history } from "../utils";
import globalReducer from "./globalSlice";
import rootSaga from "./rootsaga";

const rootReducer = combineReducers({
  router: connectRouter(history),
  global: globalReducer,
  auth: authReducer,
  dashboard: dashboardReducer,
  // student: studentReducer,
  city: cityReducer,
  category: categoryReducer,
  function: functionReducer,
});

const sagaMiddleware = createSagaMiddleware();
export const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(sagaMiddleware, routerMiddleware(history)),
});
sagaMiddleware.run(rootSaga);

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
