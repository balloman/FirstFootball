import { QueryClientProvider } from "@tanstack/react-query";
import "./App.css";
import { queryClient } from "./services/query-client";
import { AppShell} from "@mantine/core";
import { MatchList } from "./components/match-list/match-list";
import { MyHeader } from "./components/header/my-header";

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppShell header={
        <MyHeader links={[{label: "About", link: "/about"}, {label: "Me", link: "https://github.com/balloman"}]} />
      }>
        <MatchList />
      </AppShell>
    </QueryClientProvider>
  );
}

export default App;
