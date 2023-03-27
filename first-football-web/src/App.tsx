import { QueryClientProvider } from "@tanstack/react-query";
import "./App.css";
import { queryClient } from "./services/query-client";
import { AppShell} from "@mantine/core";
import { MatchList } from "./components/match-list/match-list";
import { MyHeader } from "./components/header/my-header";
import { initializeApp } from "firebase/app";
import { getAnalytics, logEvent } from "firebase/analytics";

try {
  // Your web app's Firebase configuration
  // For Firebase JS SDK v7.20.0 and later, measurementId is optional
  const firebaseConfig = {
    apiKey: "AIzaSyA20NRBDU_zuoFapREqtPqaN55O-NPnzJ4",
    authDomain: "first-football-vods.firebaseapp.com",
    projectId: "first-football-vods",
    storageBucket: "first-football-vods.appspot.com",
    messagingSenderId: "312055406415",
    appId: "1:312055406415:web:37a7a746cfc0c9db25af3a",
    measurementId: "G-YK97FD31HR"
  };

  // Initialize Firebase
  console.log("Initializing Firebase");
  const app = initializeApp(firebaseConfig);
  try {
    const analytics = getAnalytics(app);
    logEvent(analytics, "page_view");
  } catch (e) {
    console.log("Guess you blocked analytics :(");
  }
} catch (e) {
  console.log("Guess you blocked analytics :(");
}

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
