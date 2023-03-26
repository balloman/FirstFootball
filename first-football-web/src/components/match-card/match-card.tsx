import { Fixture } from "../../codegen";
import { Card, Text, Grid, Center } from "@mantine/core";

export interface MatchCardProps {
  fixture: Fixture
}

export const MatchCard = (props: MatchCardProps) => {
  const { fixture } = props;

  return (
    <Card shadow={"sm"} padding={"xs"} radius={"md"} withBorder>
      <Card.Section>
        <Center>
          {/* eslint-disable-next-line react/no-unknown-property */}
          <iframe 
            width="100%" 
            style={{aspectRatio: 1.77}} 
            src={`https://www.youtube.com/embed/${fixture.id}?controls=1&modestbranding=1&iv_load_policy=3`} 
            title="YouTube video player" 
            frameBorder="0" 
            allow="accelerometer; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" 
            allowFullScreen />
        </Center>
      </Card.Section>
      <Grid columns={11}>
        <Grid.Col span={"auto"}>
          <Center>
            <Text>{fixture.homeTeamName}</Text>
          </Center>
        </Grid.Col>
        <Grid.Col span={1}><Center><Text>v.</Text></Center></Grid.Col>
        <Grid.Col span={"auto"}>
          <Center>
            <Text>{fixture.awayTeamName}</Text>
          </Center>
        </Grid.Col>
      </Grid>
      <Text><Center><Text>{fixture.datePosted ? new Date(fixture.datePosted).toLocaleDateString() : ""}</Text></Center></Text>
    </Card>
  );
};