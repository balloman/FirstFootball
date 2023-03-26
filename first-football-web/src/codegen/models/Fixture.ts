/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Instant } from './Instant';
import type { Team } from './Team';

export type Fixture = {
  id?: string | null;
  homeTeam?: Team;
  homeTeamName?: string | null;
  awayTeam?: Team;
  awayTeamName?: string | null;
  homeScore?: number;
  awayScore?: number;
  matchWeek?: number;
  datePosted?: Instant;
};

