import { useState } from "react";
import { QueryClientProvider } from "@tanstack/react-query";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";
import { MatchCard } from "./components/match-card/match-card";
import { queryClient } from "./services/query-client";
import { AppShell, Navbar, Text, Header, Center, Anchor } from "@mantine/core";
import { MatchList } from "./components/match-list/match-list";

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppShell header={
        <Header height={{ base: 50, md: 70 }} p="md" bg={"cyan"}>
          <Center>
            <Anchor href="/" c={"white"}>FirstFootball Vods</Anchor>
          </Center>
        </Header>
      }>
        <MatchList />
      </AppShell>
    </QueryClientProvider>
  );
}

export default App;
