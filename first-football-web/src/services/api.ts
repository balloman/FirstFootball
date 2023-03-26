import ky from "ky";
import queryString from "query-string";
import { Fixture } from "../codegen";

const apiInstance = ky.create({prefixUrl: "https://backend-3iy7pdjvsa-uc.a.run.app"});

export const getFixtures = async (startAfter: Date): Promise<Fixture[]> => {
  const params = queryString.stringify({startAfter: startAfter.getTime()});
  const response = await apiInstance.get(`fixtures?${params}`);
  const fixtures = await response.json<Fixture[]>();
  return fixtures;
};