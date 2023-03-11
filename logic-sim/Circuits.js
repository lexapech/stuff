let circuits=[
    {
        name:"test",
        ports:3,
        tests: [...Array(8).keys()],
        elements: [{"name":"and_0","type":"and","pos":{"x":0.19375,"y":0.2877777777777778}},{"name":"and_1","type":"and","pos":{"x":0.35,"y":0.5844444444444444}}],
        wires:[{"startPort":{"element":"output_bar","type":"output","id":0},"endPort":{"element":"and_0","type":"input","id":0}},{"startPort":{"element":"output_bar","type":"output","id":1},"endPort":{"element":"and_0","type":"input","id":1}},{"startPort":{"element":"and_0","type":"output","id":0},"endPort":{"element":"and_1","type":"input","id":0}},{"startPort":{"element":"output_bar","type":"output","id":2},"endPort":{"element":"and_1","type":"input","id":1}},{"startPort":{"element":"and_1","type":"output","id":0},"endPort":{"element":"common_bar","type":"common","id":0}},{"startPort":{"element":"and_1","type":"output","id":0},"endPort":{"element":"common_bar","type":"common","id":1}},{"startPort":{"element":"and_1","type":"output","id":0},"endPort":{"element":"common_bar","type":"common","id":2}}]
    }
]
export default circuits