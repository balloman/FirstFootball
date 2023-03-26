/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Fixture } from '../models/Fixture';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class GetFixturesService {

  /**
   * @returns Fixture OK
   * @throws ApiError
   */
  public static getFixtures({
    limit,
    beforeMs,
  }: {
    limit?: number,
    beforeMs?: number,
  }): CancelablePromise<Array<Fixture>> {
    return __request(OpenAPI, {
      method: 'GET',
      url: '/fixtures',
      query: {
        'limit': limit,
        'beforeMs': beforeMs,
      },
      errors: {
        400: `Bad Request`,
      },
    });
  }

}
