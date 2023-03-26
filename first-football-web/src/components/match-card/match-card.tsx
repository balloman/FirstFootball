import { useQueryClient, useQuery } from "@tanstack/react-query";
import { GetFixturesService } from "../../codegen";

export const MatchCard = () => {
  GetFixturesService.getFixtures({}).then((result) => {
    console.log(result);
  });

  return <p>Hello World</p>;
};