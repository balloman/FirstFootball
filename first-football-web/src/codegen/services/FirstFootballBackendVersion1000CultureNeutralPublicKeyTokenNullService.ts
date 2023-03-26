/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class FirstFootballBackendVersion1000CultureNeutralPublicKeyTokenNullService {

  /**
   * @returns string Success
   * @throws ApiError
   */
  public static get(): CancelablePromise<string> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/',
    });
  }

}
