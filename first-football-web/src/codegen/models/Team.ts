/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Fixture } from './Fixture';

export type Team = {
  name?: string | null;
  homeFixtures?: Array<Fixture> | null;
  awayFixtures?: Array<Fixture> | null;
};

