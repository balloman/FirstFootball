import { useInfiniteQuery } from "@tanstack/react-query";
import { GetFixturesService } from "../../codegen";
import { MatchCard } from "../match-card/match-card";
import { SimpleGrid } from "@mantine/core";
import InfiniteScroll from "react-infinite-scroll-component";

export const MatchList = () => {
  const { status, data, fetchNextPage, hasNextPage } = useInfiniteQuery({
    queryKey: ["fixtures"],
    queryFn: ({ pageParam = Date.now() }) => GetFixturesService.getFixtures({limit: 20, beforeMs: pageParam}),
    getNextPageParam: (lastPage) => lastPage.length === 0 ? undefined : Date.parse(lastPage[lastPage.length - 1].datePosted as string),
  });

  if (status === "loading") {
    return (<div>Loading...</div>);
  }

  return (
    <InfiniteScroll
      dataLength={data?.pages.flat().length || 0}
      next={fetchNextPage}
      hasMore={hasNextPage || false}
      loader={<h4>Loading...</h4>}
    >
      <SimpleGrid cols={2} breakpoints={[{ maxWidth: "md", cols: 2, spacing: "md"}, { maxWidth: "sm", cols: 1, spacing: "sm"}]}>
        {data?.pages.map((page) => page.map((fixture) => <MatchCard key={fixture.id} fixture={fixture} />))}
      </SimpleGrid>
    </InfiniteScroll>
  );
};