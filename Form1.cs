namespace GraphAnalis
{
    public partial class GraphAnalis : Form
    {
        public GraphAnalis()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = richTextBox1.Text.Trim();

            if (File.Exists(filePath))
            {
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    richTextBox2.Text = fileContent;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("The specified file was not found. Check the path and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private int[,] adjacencyMatrix;
        private int verticesCount;

        private void button2_Click(object sender, EventArgs e)
        {
            // ������ ������ �� richTextBox2
            string[] lines = richTextBox2.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            verticesCount = int.Parse(lines[0].Trim());
            adjacencyMatrix = new int[verticesCount, verticesCount];

            // ���������� ������� ���������
            for (int i = 0; i < verticesCount; i++)
            {
                string[] row = lines[i + 1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < verticesCount; j++)
                {
                    adjacencyMatrix[i, j] = int.Parse(row[j]);
                }
            }

            // ���������� ��� �������
            List<string> bridges = new List<string>();
            List<string> articulationPoints = new List<string>();
            int[] entryTime = new int[verticesCount];
            int[] lowTime = new int[verticesCount];
            int[] parent = new int[verticesCount];
            bool[] visited = new bool[verticesCount];
            int timer = 0;

            for (int i = 0; i < verticesCount; i++)
            {
                parent[i] = -1; // ���������, ��� � ������� ���� ��� ��������
            }

            // ����� ������ � ����� ����������
            for (int i = 0; i < verticesCount; i++)
            {
                if (!visited[i])
                {
                    FindBridgesAndArticulationPoints(i, visited, entryTime, lowTime, parent, ref timer, bridges, articulationPoints);
                }
            }

            // ����� �����������
            richTextBox2.AppendText("\n\n");
            foreach (string bridge in bridges)
            {
                richTextBox2.AppendText(bridge + "\n");
            }

            richTextBox2.AppendText("\n");
            for (int i = 0; i < verticesCount; i++)
            {
                if (articulationPoints.Contains((i + 1).ToString()))
                {
                    richTextBox2.AppendText($"{i + 1}: ����� ����������\n");
                }
                else
                {
                    richTextBox2.AppendText($"{i + 1}: �� �������� ������ ����������\n");
                }
            }
        }

        private void FindBridgesAndArticulationPoints(
            int vertex, bool[] visited, int[] entryTime, int[] lowTime, int[] parent, ref int timer,
            List<string> bridges, List<string> articulationPoints)
        {
            visited[vertex] = true;
            entryTime[vertex] = lowTime[vertex] = ++timer;
            int children = 0;

            for (int i = 0; i < verticesCount; i++)
            {
                if (adjacencyMatrix[vertex, i] == 1)
                {
                    if (!visited[i])
                    {
                        parent[i] = vertex;
                        children++;

                        FindBridgesAndArticulationPoints(i, visited, entryTime, lowTime, parent, ref timer, bridges, articulationPoints);

                        lowTime[vertex] = Math.Min(lowTime[vertex], lowTime[i]);

                        // �������� ��� �����
                        if (lowTime[i] > entryTime[vertex])
                        {
                            bridges.Add($"����� ����� ��������� {vertex + 1} � {i + 1} - ����");
                        }

                        // �������� ��� ����� ����������
                        if (parent[vertex] == -1 && children > 1 || parent[vertex] != -1 && lowTime[i] >= entryTime[vertex])
                        {
                            if (!articulationPoints.Contains((vertex + 1).ToString()))
                            {
                                articulationPoints.Add((vertex + 1).ToString());
                            }
                        }
                    }
                    else if (i != parent[vertex])
                    {
                        lowTime[vertex] = Math.Min(lowTime[vertex], entryTime[i]);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
        }
    }
}
