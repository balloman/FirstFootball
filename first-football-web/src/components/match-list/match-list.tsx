import { useQueryClient, useInfiniteQuery } from "@tanstack/react-query";
import { GetFixturesService } from "../../codegen";
import { MatchCard } from "../match-card/match-card";
import { SimpleGrid } from "@mantine/core";

export const MatchList = () => {
  const queryClient = useQueryClient();

  const { status, data, fetchNextPage, hasNextPage } = useInfiniteQuery({
    queryKey: ["fixtures"],
    queryFn: ({ pageParam = Date.now() }) => GetFixturesService.getFixtures({beforeMs: pageParam}),
    getNextPageParam: (lastPage) => lastPage.length === 0 ? undefined : Date.parse(lastPage[lastPage.length - 1].datePosted as string),
  });
  GetFixturesService.getFixtures({}).then((result) => {
    console.log(result);
  });

  if (status === "loading") {
    return (<div>Loading...</div>);
  }

  return (
    <SimpleGrid cols={2}>
      {data?.pages.map(page => page.map(fixture => (
        <div key={fixture.id}>
          <MatchCard fixture={fixture} />
        </div>)))}
    </SimpleGrid>
  );
};